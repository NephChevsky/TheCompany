import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';
import { CustomerEntity } from '../_models/customerEntity';

@Injectable({ providedIn: 'root' })
export class CustomerEntityService
{
	constructor(private http: HttpClient)
	{
	}

	createCustomer(customer: CustomerEntity)
	{
		return this.http.post<CustomerEntity>(environment.baseUrl + `CustomerEntity/Create`, customer);
	}
}
