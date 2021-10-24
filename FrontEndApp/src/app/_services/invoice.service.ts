import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class InvoiceService
{
	constructor(private http: HttpClient)
	{
		
	}

	import(data: any)
	{
		return this.http.post(environment.baseUrl + `Invoice/Import`, data, {
		reportProgress: true,
		observe: 'events'
		})
	}
}
