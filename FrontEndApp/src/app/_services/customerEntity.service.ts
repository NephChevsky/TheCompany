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

	save(customer: any)
	{
		return this.http.post(environment.baseUrl + `Customer/Save`, customer);
	}

	get(id: string)
	{
		return this.http.get(environment.baseUrl + `Customer/Show/` + id);
	}
}
