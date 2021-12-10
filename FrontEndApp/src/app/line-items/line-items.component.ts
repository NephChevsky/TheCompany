import { Component, OnInit } from '@angular/core';

@Component({
	selector: 'app-line-items',
	templateUrl: './line-items.component.html',
	styleUrls: ['./line-items.component.scss']
})
export class LineItemsComponent implements OnInit
{
	public dataSource: string = "LineItemDefinition";
	public linkField: string = "Id";
	public linkRoute: string = "LineItems/Show/"

	constructor() { }

	ngOnInit(): void
	{
	}

}
