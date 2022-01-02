import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
	selector: 'app-customers',
	templateUrl: './customers.component.html',
	styleUrls: ['./customers.component.scss']
})
export class CustomersComponent implements OnInit
{
	public dataSource: string = "Individual";
	public linkRoute: string = "Customers/Show/"
	constructor(private router: Router) { }

	ngOnInit(): void
	{
	}
}
