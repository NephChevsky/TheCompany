import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomerEntityService } from 'src/app/_services/customerEntity.service';

@Component({
	selector: 'app-create-customer',
	templateUrl: './create-customer.component.html',
	styleUrls: ['./create-customer.component.scss']
})
export class CreateCustomerComponent implements OnInit {

	customerForm: FormGroup = new FormGroup({});

	constructor(private formBuilder: FormBuilder, 
				private customerEntityService: CustomerEntityService,
				private router: Router)
	{

	}

	ngOnInit(): void
	{
		this.customerForm = this.formBuilder.group({
			customerNumber: ['', [Validators.required]],
			lastName: ['', [Validators.required]],
			firstName: ['', []],
			email: ['', [Validators.email]],
			phoneNumber: ['', []],
			mobilePhoneNumber: ['', []],
			address: ['', []]
		});
	}

	onSubmit()
	{
		if (this.customerForm.invalid)
		{
			return;
		}
		this.customerEntityService.createCustomer(this.customerForm.value)
			.subscribe(
				data =>
				{
					this.router.navigate(['Customers']);
					return;
				},
				error =>
				{
					/* TODO: set errors on field */
				});
	}

	onCancel()
	{
		this.router.navigate(['Customers']);
	}
}
