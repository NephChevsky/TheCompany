export class User {
    public id: string = "";
    public login: string = "";
	public email: string = "";
    public password: string = "";
    public token: string = "";
	public LastLoginDateTime: Date = new Date();
	public CreationDateTime: Date = new Date();
	public LastModificationDateTime: Date = new Date();
}
