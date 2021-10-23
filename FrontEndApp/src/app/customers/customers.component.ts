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
	constructor(private router: Router) { }

	ngOnInit(): void {
	}

	createCustomer() {
		this.router.navigate(['customers/create']);
	}

}
