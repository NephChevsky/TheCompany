import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LineItemService
{
	constructor(private http: HttpClient)
	{
	}

	get(data: any)
	{
		return this.http.get(environment.baseUrl + `LineItem/Show/` + data);
	}

	create(data: any)
	{
		return this.http.post(environment.baseUrl + `LineItem/Create`, data);
	}

	save(data: any)
	{
		return this.http.post(environment.baseUrl + `LineItem/Save`, data);
	}
}
