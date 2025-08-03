"use client";

import {
	Area,
	AreaChart,
	CartesianGrid,
	Label,
	ReferenceLine,
	XAxis,
	YAxis,
} from "recharts";

import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";
import {
	ChartConfig,
	ChartContainer,
	ChartTooltip,
	ChartTooltipContent,
} from "@/components/ui/chart";
import Summary from "@/types/summary";

export const description = "A linear area chart";

const chartConfig = {
	desktop: {
		label: "Desktop",
		color: "var(--chart-1)",
	},
} satisfies ChartConfig;

export function ChartAreaLinear({ data }: { data?: Summary }) {
	return (
		<Card className="w-full">
			<CardHeader>
				<CardTitle>Monthly Spending</CardTitle>
				<CardDescription>
					Showing current and projected spending for{" "}
					{new Date().toLocaleString("default", { month: "long" })}
				</CardDescription>
			</CardHeader>
			<CardContent>				
				<ChartContainer config={chartConfig}>
					<AreaChart
						accessibilityLayer
						data={data?.line_graph_expenses}						
					>
						<CartesianGrid vertical={false} />
						<XAxis
							dataKey="day"
							tickLine={false}
							axisLine={false}
							tickMargin={8}
							tickFormatter={(value) => value % 2 == 0 ? "" : value}
						/>
						<YAxis
							domain={[
								0,
								Math.ceil((data?.amountProjected ?? 0) / 50.0) * 50.0,
							]}
							tickLine={false}
							axisLine={false}
							tickMargin={8}
							style={(data?.amountSpent === 0) ? {display: 'none'} : {}}							
						/>
						<ReferenceLine
							y={data?.amountProjected}
							stroke={(data?.amountSpent === 0) ? "" : "red"}
							strokeDasharray="4 4"
						>
							<Label
								value={(data?.amountSpent === 0) ? "Upload a receipt for this month to start tracking" : "Forecasted Monthly Cost"}
								position="top"
								fill="white"
								fontSize={12}
								fontWeight="bold"
								textAnchor="middle"
							/>
						</ReferenceLine>

						{(data?.amountSpent !== 0) && <ChartTooltip
							cursor={false}
							content={<ChartTooltipContent indicator="dot" hideLabel />}
						/>}
						<Area
							dataKey="totalAmount"
							type="linear"
							fill="var(--color-desktop)"
							fillOpacity={0.4}
							style={(data?.amountSpent === 0) ? {display: 'none'} : {}}
							stroke="var(--color-desktop)"
							activeDot={data?.amountSpent !== 0}
						/>
					</AreaChart>
				</ChartContainer>
			</CardContent>
			{/*<CardFooter>
        <div className="flex w-full items-start gap-2 text-sm">
          <div className="grid gap-2">
            <div className="flex items-center gap-2 leading-none font-medium">
              Trending up by 5.2% this month <TrendingUp className="h-4 w-4" />
            </div>
            <div className="text-muted-foreground flex items-center gap-2 leading-none">
              January - June 2024
            </div>
          </div>
        </div>
      </CardFooter>*/}
		</Card>
	);
}
