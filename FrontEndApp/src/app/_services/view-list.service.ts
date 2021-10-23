import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ViewListService
{
	constructor(private http: HttpClient)
	{
		
	}

	getResults(dataSource: string)
	{
		return this.http.post<any>(environment.baseUrl + `ViewList/Get`, {dataSource: dataSource});
	}
}
