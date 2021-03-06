import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  formData: FormData;

  constructor(private httpClient: HttpClient) { }
 
  getMethodUserInfo(): Observable<any>{
    return this.httpClient.get(" https://localhost:44365/api/Account/UserInfo");
  }

  getMethodUsersForApproves() : Observable<any> {
    return this.httpClient.get(" https://localhost:44365/api/Account/AccountsForApproval");
  }

  postMethodApproveUser(id : number) {
    return this.httpClient.post("https://localhost:44365/api/Account/ApproveAccount", id);
  }

  postMethodRejectUser(id : number) {
    return this.httpClient.post("https://localhost:44365/api/Account/RejectAccount", id);
  }

  getMethodUsers(){
    return this.httpClient.get("https://localhost:44365/api/Account/GetUsers");
  }

  getMethodManagers(){
    return this.httpClient.get("https://localhost:44365/api/Account/GetManagers");
  }

  getMethodRoles(){
    return this.httpClient.get("https://localhost:44365/api/Account/GetRoles");
  }

  putMethodChangeRole(userId, role){
    return this.httpClient.put("https://localhost:44365/api/Account/ChangeRole?userId=" + userId, role);
  }

  putMethodManagers(managerId){
    this.formData = new FormData();
    
    this.formData.append('managerId', managerId);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put("https://localhost:44365/api/Account/ChangeManagerBan", this.formData, { headers: headers });
  }
}
