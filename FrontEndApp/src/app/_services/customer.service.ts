import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';
import { CustomerEntity } from '../_models/customerEntity';

@Injectable({ providedIn: 'root' })
export class CustomerService
{
	constructor(private http: HttpClient)
	{
	}

	createCustomer(customer: CustomerEntity)
	{
		this.http.post<CustomerEntity>(environment.baseUrl + `Customer/Create`, customer);
	}
}
