import { Injectable } from '@angular/core';

import { Http, Response } from '@angular/http';
import { Headers, RequestOptions } from '@angular/http';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class BranchOfficeService {

  constructor(private httpClient: HttpClient) { }

  postMethodCreateBranchOffice(branchOffice): Observable<any> {
    return this.httpClient.post("http://localhost:51680/api/BranchOffices", branchOffice)
  }
  
}
