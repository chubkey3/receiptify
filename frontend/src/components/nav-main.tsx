"use client";

import {
	IconCirclePlusFilled,
	IconReceipt,
	type Icon,
} from "@tabler/icons-react";

import { Button } from "@/components/ui/button";
import {
	SidebarGroup,
	SidebarGroupContent,
	SidebarMenu,
	SidebarMenuButton,
	SidebarMenuItem,
} from "@/components/ui/sidebar";
import {
	Drawer,
	DrawerClose,
	DrawerContent,
	DrawerFooter,
	DrawerHeader,
	DrawerTitle,
	DrawerTrigger,
} from "./ui/drawer";
import { Separator } from "./ui/separator";
import { Label } from "@radix-ui/react-label";
import { Input } from "./ui/input";
import { useState } from "react";
import axios from "@/util/axios";
import { KeyedMutator } from "swr";
import { toast } from "sonner";
import Summary from "@/types/summary";
import Expense from "@/types/expense";

export function NavMain({
	items,
	analyticsMutate,
	expensesMutate,
}: {
	items: {
		title: string;
		url: string;
		icon?: Icon;
	}[];
	analyticsMutate: KeyedMutator<{ value: Summary }>;
	expensesMutate: KeyedMutator<Expense[]>;
}) {
	return (
		<SidebarGroup>
			<SidebarGroupContent className="flex flex-col gap-2">
				<SidebarMenu>
					<SidebarMenuItem className="flex items-center gap-2">
						<UploadDrawer
							analyticsMutate={analyticsMutate}
							expensesMutate={expensesMutate}
						/>
						{/*<Button
              size="icon"
              className="size-8 group-data-[collapsible=icon]:opacity-0"
              variant="outline"
            >
              <IconMail />
              <span className="sr-only">Inbox</span>
            </Button>*/}
					</SidebarMenuItem>
				</SidebarMenu>
				<SidebarMenu>
					{items.map((item) => (
						<SidebarMenuItem key={item.title}>
							<SidebarMenuButton tooltip={item.title}>
								{item.icon && <item.icon />}
								<span>{item.title}</span>
							</SidebarMenuButton>
						</SidebarMenuItem>
					))}
				</SidebarMenu>
			</SidebarGroupContent>
		</SidebarGroup>
	);
}

function UploadDrawer({
	analyticsMutate,
	expensesMutate,
}: {
	analyticsMutate: KeyedMutator<{ value: Summary }>;
	expensesMutate: KeyedMutator<Expense[]>;
}) {
	const [file, setFile] = useState<File>();
	const [isLoading, toggleLoading] = useState<boolean>(false);

	const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
		if (event.target.files) {
			// test if image is valid
			const file = event.target.files[0];
			const img = new Image();
			const imgURL = URL.createObjectURL(file);
			img.src = imgURL;

			img.onload = () => {
				setFile(file);
			};
			img.onerror = () => {
				setFile(undefined);
			};
		}
	};

	const handleUpload = async () => {
		if (!file) {
			toast.error("No image found to upload.", { position: "top-center" });
			return;
		}
		toggleLoading(true);
		let response;
		const formData = new FormData();

		formData.append("file", file);

		try {
			response = await axios.post("/upload", formData);
		} catch (error) {
			toast.error("Image upload failed!", {
				position: "top-center",
				description: `Status Code: ${response?.status} | Error: ${error}`,
			});
		}
		toggleLoading(false);
		if (response && response.status === 200) {
			//update data
			analyticsMutate();
			expensesMutate();

			toast.success("Receipt uploaded successfully!", {
				position: "top-center",
			});
		} else {
			toast.error("Image upload failed!", {
				position: "top-center",
				description: `Status Code: ${response?.status} | Error: ${response?.statusText}`,
			});
		}
	};

	return (
		<Drawer direction={"bottom"}>
			<DrawerTrigger asChild>
				<SidebarMenuButton
					tooltip="Upload Receipt"
					className="bg-primary text-primary-foreground hover:bg-primary/90 hover:text-primary-foreground active:bg-primary/90 active:text-primary-foreground min-w-8 duration-200 ease-linear"
				>
					<IconCirclePlusFilled />
					<span>Upload Receipt</span>
				</SidebarMenuButton>
			</DrawerTrigger>
			<DrawerContent>
				<DrawerHeader className="gap-1">
					<DrawerTitle>{"Upload Receipt"}</DrawerTitle>
				</DrawerHeader>
				<div className="flex flex-col gap-4 overflow-y-auto px-4 text-sm">
					<>
						<Separator />
						<div className="grid gap-2">
							<div className="flex gap-2 leading-none font-medium">
								Upload an image of a receipt or take a photo of one
								<IconReceipt className="size-4" />
							</div>
							<div className="text-muted-foreground">
								Make sure your receipt is taken in good lighting conditions
								where all text is visible.
							</div>
						</div>
						<Separator />
					</>

					<form className="flex flex-col gap-4">
						<div className="grid w-full max-w-sm items-center gap-3 mb-[25px]">
							<Label htmlFor="picture">Image</Label>
							<Input onChange={handleFileChange} accept="image/*" type="file" />
						</div>
					</form>
				</div>
				<DrawerFooter>
					<Button onClick={handleUpload}>
						{isLoading ? (
							<div
								className={`h-5 w-5 animate-spin rounded-full border-2 border-muted-foreground border-t-transparent`}
							/>
						) : (
							"Upload"
						)}
					</Button>
					<DrawerClose asChild>
						<Button variant="outline">Done</Button>
					</DrawerClose>
				</DrawerFooter>
			</DrawerContent>
		</Drawer>
	);
}
