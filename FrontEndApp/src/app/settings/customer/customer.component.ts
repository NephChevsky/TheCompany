import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.scss']
})
export class CustomerComponent implements OnInit
{
	public dataSource: string = "AdditionalField";

	constructor(private router: Router) { }

	ngOnInit(): void {
	}

	addAdditionalField()
	{
		this.router.navigate(["AdditionalFields/Add/Customer"]);
	}
}
