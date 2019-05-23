import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  private _baseApiUri: string = 'https://localhost:44334';

  constructor() { }

  // 登录用Uri
  loginUri() {
    return this._baseApiUri + '/login';
  }


}
