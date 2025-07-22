import { IconTrendingDown, IconTrendingUp } from "@tabler/icons-react";

import { Badge } from "@/components/ui/badge";
import {
	Card,
	CardAction,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";
import Summary from "@/types/summary";
import { Skeleton } from "./ui/skeleton";
import { ChartAreaLinear } from "./chart-area-linear";
import { ChartPieLegend } from "./chart-pie-legend";

function PercentChangeComponent(isLoading: boolean, data?: number) {
	if (isLoading) {
		return (
			<Badge variant="outline">
				<IconTrendingUp />
				<Skeleton className="h-[16px] w-[46px]" />
			</Badge>
		);
	}
	if (!data) {
		return (
			<Badge variant="outline">
				<IconTrendingUp />
				N/A
			</Badge>
		);
	}
	if (data < 0) {
		return (
			<Badge variant="outline">
				<IconTrendingDown />
				{data.toFixed(2)}%
			</Badge>
		);
	} else {
		return (
			<Badge variant="outline">
				<IconTrendingUp />
				{data.toFixed(2)}%
			</Badge>
		);
	}
}
export function SectionCards({
	data,
	isLoading,
}: {
	data?: Summary;
	isLoading: boolean;
}) {
	return (
		<div className="*:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card dark:*:data-[slot=card]:bg-card grid grid-cols-1 gap-4 px-4 *:data-[slot=card]:bg-gradient-to-t *:data-[slot=card]:shadow-xs lg:px-6 @xl/main:grid-cols-2 @5xl/main:grid-cols-4">
			<Card className="@container/card">
				<CardHeader>
					<CardDescription>Total Spending</CardDescription>
					<CardTitle className="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
						{isLoading ? (
							<Skeleton className="h-[35px] w-[165px]" />
						) : (
							data?.amountSpent.toFixed(2)
						)}
					</CardTitle>
					<CardAction>
						{PercentChangeComponent(isLoading, data?.percentChangeTotal)}
					</CardAction>
				</CardHeader>
				<CardFooter className="flex-col items-start gap-1.5 text-sm">
					<div className="text-muted-foreground">
						Amount spent in{" "}
						{new Date().toLocaleString("default", { month: "long" })}
					</div>
				</CardFooter>
			</Card>
			<Card className="@container/card">
				<CardHeader>
					<CardDescription>Projected Total Spending</CardDescription>
					<CardTitle className="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
						{isLoading ? (
							<Skeleton className="h-[35px] w-[165px]" />
						) : (
							data?.amountProjected.toFixed(2)
						)}
					</CardTitle>
					<CardAction>
						{PercentChangeComponent(isLoading, data?.percentChangeProjected)}
					</CardAction>
				</CardHeader>
				<CardFooter className="flex-col items-start gap-1.5 text-sm">
					<div className="text-muted-foreground">
						Projected amount spent throughout{" "}
						{new Date().toLocaleString("default", { month: "long" })}
					</div>
				</CardFooter>
			</Card>
			<Card className="@container/card">
				<CardHeader>
					<CardDescription>Top Supplier</CardDescription>
					<CardTitle className="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
						{isLoading ? (
							<Skeleton className="h-[35px] w-[165px]" />
						) : (
							data?.topMerchant
						)}
					</CardTitle>
				</CardHeader>
				<CardFooter className="flex-col items-start gap-1.5 text-sm">
					<div className="text-muted-foreground">
						Top supplier by spending in{" "}
						{new Date().toLocaleString("default", { month: "long" })}
					</div>
				</CardFooter>
			</Card>
			<Card className="@container/card">
				<CardHeader>
					<CardDescription>Top Expense</CardDescription>
					<CardTitle className="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
						{isLoading ? (
							<Skeleton className="h-[35px] w-[165px]" />
						) : (
							data?.topMerchantAmount.toFixed(2)
						)}
					</CardTitle>
				</CardHeader>
				<CardFooter className="flex-col items-start gap-1.5 text-sm">
					<div className="text-muted-foreground">
						Highest expense amount in{" "}
						{new Date().toLocaleString("default", { month: "long" })}
					</div>
				</CardFooter>
			</Card>
			<ChartAreaLinear data={data} />
			<ChartPieLegend data={data} />
		</div>
	);
}
