import { Bindings } from "./bindings";

export class Field
{
	public dataSource: string = "";
	public name: string = "";
	public type: string = "";
	public value: string = "";
	public possibleValues: string[] = [];
	public editable: boolean = false;
	public autoCompletable: boolean = false;
	public bindings: Bindings;

	constructor()
	{
	}
}
