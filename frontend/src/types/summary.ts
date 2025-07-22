export default interface Summary {
	amountSpent: number;
	amountProjected: number;
	topMerchant: string;
	topMerchantAmount: number;
	line_graph_expenses: [
		{
			day: number;
			totalAmount: number;
		},
	];
	circle_graph_expenses: [
		{
			supplierName: string;
			totalAmount: number;
			grandTotal: number;
		},
	];
	percentChangeTotal: number;
	percentChangeProjected: number;
}
