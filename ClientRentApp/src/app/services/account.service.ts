import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private httpClient: HttpClient) { }

  getMethodUserInfo(): Observable<any>{
    return this.httpClient.get("http://localhost:51680/api/Account/UserInfo");
  }

  getMethodUsersForApproves() : Observable<any> {
    return this.httpClient.get("http://localhost:51680/api/Account/AccountsForApproval");
  }

  postMethodApproveUser(id : number) {
    return this.httpClient.post("http://localhost:51680/api/Account/ApproveAccount", id);
  }

  postMethodRejectUser(id : number) {
    return this.httpClient.post("http://localhost:51680/api/Account/RejectAccount", id);
  }
}
