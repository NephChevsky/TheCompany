export class Filter {
    public FieldName: string = "";
	public Operator: string = "";
	public FieldValue: string = "";

	constructor(fieldName: string, operator: string, fieldValue: string)
	{
		this.FieldName = fieldName;
		this.Operator = operator;
		this.FieldValue = fieldValue;
	}
}