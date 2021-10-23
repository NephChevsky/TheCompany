import { Address } from "./address";

export class CustomerEntity {
    public id: string = "";
	public Type: number = 0;
	public CreationDateTime: Date = new Date();
	public LastModificationDateTime: Date = new Date();
}

export class Individual extends CustomerEntity
{
	public firstName: string = "";
	public lastName: string = "";
	public email: string = "";
	public phoneNumber: string = "";
	public mobilePhoneNumber: string = "";
	public address: Address = new Address();
}