import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Filter } from 'src/app/_models/filter';

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.scss']
})
export class InvoiceComponent implements OnInit {

	public dataSource: string = "AdditionalField";
	public filters: Filter[] = [new Filter("DataSource", "=", "Invoice")];

	constructor(private router: Router) { }

	ngOnInit(): void
	{
	}

	addAdditionalField()
	{
		this.router.navigate(["AdditionalFields/Add/Invoice"]);
	}
}
