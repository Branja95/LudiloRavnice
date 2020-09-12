import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { environment } from '../../environments/environment'
import { User } from '../models/user.model'
import { env } from 'process';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  formData: FormData;

  constructor(private httpClient: HttpClient) { }
 
  getMethodUserInfo(): Observable<any>{
    return this.httpClient.get(environment.endpointAccountUserInfo);
  }

  getMethodUsersForApproves() : Observable<any> {
    return this.httpClient.get(environment.endpointAccountsForApproval);
  }

  postMethodApproveUser(id : number) {
    console.log(environment.endpointAccountApproveAccount);
    console.log(id);
    return this.httpClient.post(environment.endpointAccountApproveAccount, id);
  }

  postMethodRejectUser(id : number) {
    return this.httpClient.post(environment.endpointAccountRejectAccount, id);
  }

  getMethodUsers(){
    return this.httpClient.get(environment.endpointAccountGetUsers);
  }

  getMethodManagers(){
    return this.httpClient.get(environment.endpointAccountGetManagers);
  }

  getMethodRoles(){
    return this.httpClient.get(environment.endpointAccountGetRoles);
  }

  putMethodChangeRole(id: string, role: any){
    return this.httpClient.put(environment.endpointAccountChangeRole + id, role);
  }

  putMethodManagers(id: string | Blob){
    this.formData = new FormData();
    this.formData.append('ManagerId', id);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put(environment.endpointAccountChangeManagerBan, this.formData, { headers: headers });
  }
}
