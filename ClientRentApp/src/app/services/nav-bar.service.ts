import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class NavBarService {

  constructor(private httpClient: HttpClient) { }

  getMethodIsUserApproved() : Observable<any> {
    return this.httpClient.get("http://localhost:51680/api/Account/IsUserApproved");
  }

  postMethodLogout() {
    return this.httpClient.post("http://localhost:51680/api/Account/Logout", "");
  }
}
