export default interface Expense {
	expenseId: number;
	totalAmount: number;
	expenseDate: Date;
	uid: string;
	receiptId: number;
	supplierId: number;
	receipt: null;
	supplier: {
		supplierId: number;
		supplierName: string;
		expenses: [];
	};
	uidNavigation: null;
}
