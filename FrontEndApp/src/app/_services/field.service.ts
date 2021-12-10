import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class FieldService
{
	constructor(private http: HttpClient)
	{
	}

	getFile(data: any)
	{
		return this.http.get(environment.baseUrl + `Field/GetFile/` + data, { responseType: 'arraybuffer' });
	}
}
