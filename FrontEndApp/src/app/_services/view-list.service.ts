import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';
import { Filter } from '../_models/filter';

@Injectable({ providedIn: 'root' })
export class ViewListService
{
	constructor(private http: HttpClient)
	{
		
	}

	getResults(dataSource: string, filters: Filter[], fields: string[], linkField: string)
	{
		var obj = {
			dataSource: dataSource,
			filters: filters,
			fields: fields,
			linkField: linkField
		}
		return this.http.post<any>(environment.baseUrl + `ViewList/Get`, obj);
	}
}
