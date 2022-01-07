import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Filter } from 'src/app/_models/filter';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.scss']
})
export class CustomerComponent implements OnInit
{
	public dataSource: string = "AdditionalFieldDefinition";
	public filters: Filter[] = [new Filter("DataSource", "=", "Customer")];

	constructor(private router: Router) { }

	ngOnInit(): void {
	}

	addAdditionalField()
	{
		this.router.navigate(["AdditionalFields/Add/Customer"]);
	}
}
