import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AdditionalFieldService
{
	constructor(private http: HttpClient)
	{
	}

	createField(data: any)
	{
		return this.http.post(environment.baseUrl + `AdditionalField/Add`, data);
	}

	getFields(data: string)
	{
		return this.http.get<string[]>(environment.baseUrl + `AdditionalField/Get/` + data);
	}
}
