"use client"

import { Cell, Pie, PieChart } from "recharts"

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import {
  ChartConfig,
  ChartContainer,
  ChartLegend,  
} from "@/components/ui/chart"
import Summary from "@/types/summary"

export const description = "A pie chart with a legend"

const COLORS = [
  "var(--chart-1)",
  "var(--chart-2)",
  "var(--chart-3)",
  "var(--chart-4)",
  "var(--chart-5)",
]


const chartConfig = {
  totalAmount: {
    label: "Visitors",
  },
  chrome: {
    label: "Stong MARKET",
    color: "var(--chart-1)",
  },
  safari: {
    label: "Chickasta Master Brand",
    color: "var(--chart-2)",
  },
  firefox: {
    label: "Firefox",
    color: "var(--chart-3)",
  },
  edge: {
    label: "Edge",
    color: "var(--chart-4)",
  },
  other: {
    label: "Other",
    color: "var(--chart-5)",
  },
} satisfies ChartConfig

export function ChartPieLegend({data}: {data?: Summary}) {  
  return (
    <Card className="flex flex-col w-full">
      <CardHeader className="items-center pb-0">
        <CardTitle>Monthly Suppliers</CardTitle>
        <CardDescription>Showing suppliers and their share of spending for {(new Date()).toLocaleString('default', {month: 'long'})}</CardDescription>
      </CardHeader>
      <CardContent className="flex-1 pb-0">
        <ChartContainer
          config={chartConfig}
          className="mx-auto aspect-square max-h-[300px]"
        >
          <PieChart>
            <Pie data={data?.circle_graph_expenses} dataKey="totalAmount" nameKey="supplierName">
              {data?.circle_graph_expenses?.map((entry, index) => (
    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
  ))}
              </Pie>
            <ChartLegend
              
              className="-translate-y-2 flex-wrap gap-2 *:basis-1/4 *:justify-center"
            />
          </PieChart>
        </ChartContainer>
      </CardContent>
    </Card>
  )
}
