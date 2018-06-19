import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { BranchOffice } from '../models/branch-office.model';

@Injectable({
  providedIn: 'root'
})
export class BranchOfficeService {

  formData: FormData = new FormData();
  
  constructor(private httpClient: HttpClient) { }

  postMethodCreateBranchOffice(branchOffice, uploadedImage: File): Observable<any> {
    
    this.formData.append('address', branchOffice.address)
    this.formData.append('latitude', branchOffice.latitude);
    this.formData.append('longitude', branchOffice.longitude);
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result = this.httpClient.post("http://localhost:51680/api/BranchOffices",this.formData, { headers: headers });

    this.formData = new FormData();

    return result;
   }

   getMethodBranchOffices(): Observable<BranchOffice[]> {
    return this.httpClient.get<BranchOffice[]>("http://localhost:51680/api/BranchOffices")
  }
}

