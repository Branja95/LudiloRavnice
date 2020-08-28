import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NavBarService {

  constructor(private httpClient: HttpClient) { }

  getMethodIsUserApproved() : Observable<any> {
    return this.httpClient.get(environment.endpointAccountIsUserApproved);
  }

  postMethodLogout() {
    return this.httpClient.post(environment.endpointAccountLogout, "");
  }

  getMethodAccountForApproval(){
    return this.httpClient.get(environment.endpointAccountForApproval);
  }

  getMethodServiceForApproval(){
    return this.httpClient.get(environment.endpointRentVehicleServicesForApprovalCount);
  }
}
