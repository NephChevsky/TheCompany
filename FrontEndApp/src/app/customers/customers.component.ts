import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.scss']
})
export class CustomersComponent implements OnInit
{
	constructor(private router: Router) { }

	ngOnInit(): void {
	}

	createCustomer(show: boolean) {
		this.router.navigate(['customers/create']);
	}

}
