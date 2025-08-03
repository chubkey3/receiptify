import Link from "next/link";
import { Button } from "@/components/ui/button";
import FeatureCard from "@/components/feature-card";
import axios from "axios";
import { redirect } from "next/navigation";
import { cookies } from "next/headers";
export default async function Home() {
	const loggedIn = false;
	const cookieStore = cookies();

	let status = 0;

	try {
		const res = await axios.get(`${process.env.NEXT_PUBLIC_API_URL}user`, {
			headers: {
				Cookie: (await cookieStore).toString(),
			},
			validateStatus: () => true,
		});

		if (res.status === 200) {
			status = 200;
		}
	} catch (err) {
		console.error("Failed to check user auth:", err);
	}

	if (status === 200) {
		redirect("/dashboard");
	}

	return (
		<div>
			<header className="w-full border-b border-gray-200 h-18 flex justify-between px-5 items-center">
				<div className="w-full px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
					<div className="ml-2 text-xl font-semibold">Receiptify</div>
					<div>
						{!loggedIn ? (
							<div className="flex items-center space-x-4">
								<Link
									href="/login"
									className="text-sm font-medium text-gray-700 hover:text-gray-900"
								>
									Log In
								</Link>

								<Link
									href="/register"
									className="inline-flex items-center justify-center gap-2 whitespace-nowrap text-sm font-medium transition-all disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 shrink-0 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive bg-primary text-primary-foreground shadow-xs hover:bg-primary/90 h-9 px-4 py-2 has-[>svg]:px-3 rounded-full"
								>
									Sign Up
								</Link>
							</div>
						) : (
							<Link
								href="/dashboard"
								className="inline-flex items-center justify-center gap-2 whitespace-nowrap text-sm font-medium transition-all disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 shrink-0 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive bg-primary text-primary-foreground shadow-xs hover:bg-primary/90 h-9 px-4 py-2 has-[>svg]:px-3 rounded-full"
							>
								Dashboard
							</Link>
						)}
					</div>
				</div>
			</header>

			<main>
				<div className="py-20 bg-white flex justify-center">
					<div className="max-w-2xl flex justify-center flex-col items-center">
						<h1 className="text-4xl font-bold text-gray-900 tracking-tight sm:text-5xl md:text-6xl">
							Build Your SaaS
							<span className="block text-orange-500">Faster Than Ever</span>
						</h1>
						<p className="mt-3 text-base text-gray-500 sm:mt-5 sm:text-xl lg:text-lg xl:text-xl">
							Launch your SaaS product in record time with our powerful,
							ready-to-use template. Packed with modern technologies and
							essential integrations.
						</p>
						<div className="mt-8 sm:max-w-lg sm:mx-auto sm:text-center lg:text-left lg:mx-0">
							<Button>Try it Now!</Button>
						</div>
					</div>
				</div>

				<div className="py-20 bg-gray-200 flex justify-center">
					<div className="flex">
						<FeatureCard />
					</div>
				</div>
			</main>
		</div>
	);
}
