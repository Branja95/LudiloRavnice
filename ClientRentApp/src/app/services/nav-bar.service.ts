import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class NavBarService {

  constructor(private httpClient: HttpClient) { }

  getMethodIsUserApproved() : Observable<any> {
    return this.httpClient.get("https://localhost:5001/api/Account/IsUserApproved");
  }

  postMethodLogout() {
    return this.httpClient.post("https://localhost:5001/api/Auth/Logout", "");
  }

  getMethodAccountForApproval(){
    console.log('usao');
    return this.httpClient.get("https://localhost:5001/api/Account/AccountForApproval");
  }

  getMethodServiceForApproval(){
    return this.httpClient.get("https://localhost:5001/api/Account/ServiceForApproval");
  }
}
