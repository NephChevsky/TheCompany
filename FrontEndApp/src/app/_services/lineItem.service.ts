import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LineItemService
{
	constructor(private http: HttpClient)
	{
	}

	getFields()
	{
		return this.http.get<any>(environment.baseUrl + `LineItem/GetFields`);
	}

	create(data: any)
	{
		return this.http.post(environment.baseUrl + `LineItem/Create`, data);
	}
}
