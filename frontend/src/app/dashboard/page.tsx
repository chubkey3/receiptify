"use client";
import { AppSidebar } from "@/components/app-sidebar";
import { DataTable } from "@/components/data-table";
import { SectionCards } from "@/components/section-cards";
import { SiteHeader } from "@/components/site-header";
import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar";

import axios from "@/util/axios";
import useSWR from "swr";
import Summary from "@/types/summary";
import Expense from "@/types/expense";

export default function Page() {
	const fetcher = (url: string) => axios.get(url).then((res) => res.data);

	const { data, error, isLoading, mutate } = useSWR<{ value: Summary }>(
		"/analytics/summary",
		fetcher,
	);
	const expenses = useSWR<Expense[]>("/user/expenses?pageNumber=1", fetcher);

	//axios.get('analytics/summary')
	if (error) return <p>Error: {error.message}</p>;

	return (
		<SidebarProvider
			style={
				{
					"--sidebar-width": "calc(var(--spacing) * 72)",
					"--header-height": "calc(var(--spacing) * 12)",
				} as React.CSSProperties
			}
		>
			<AppSidebar
				variant="inset"
				analyticsMutate={mutate}
				expensesMutate={expenses.mutate}
			/>
			<SidebarInset>
				<SiteHeader />
				<div className="flex flex-1 flex-col">
					<div className="@container/main flex flex-1 flex-col gap-2">
						<div className="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
							<SectionCards data={data?.value} isLoading={isLoading} />
							<DataTable
								data={expenses.data ? expenses.data : []}
								analyticsMutate={mutate}
								expensesMutate={expenses.mutate}
							/>
						</div>
					</div>
				</div>
			</SidebarInset>
		</SidebarProvider>
	);
}
