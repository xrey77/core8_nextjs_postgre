class appConfig {
  private _userid: string;
  private _token: string;

  constructor(initialValue1: string, initialValue2: string) {
    this._userid = initialValue1;
    this._token = initialValue2;
  }

  get useridValue(): string {
    return this._userid;
  }

  get tokenValue(): string {
    return this._token;
  }
}