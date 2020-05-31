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

  getBranchOffice(branchOfficeId): Observable<any> {
    return this.httpClient.get("https://localhost:44367/api/BranchOffice/GetBranchOffice?branchOfficeId=" + branchOfficeId);
  }

  getBranchOffices(serviceId): Observable<any> {
    return this.httpClient.get("https://localhost:44367/api/Service/GetBranchOffices?serviceId=" + serviceId);
  }

  getVehicleServiceBranchOffices(vehicleId): Observable<any> {
    return this.httpClient.get("https://localhost:44367/api/BranchOffice/GetVehicleBranchOffices?vehicleId=" + vehicleId);
  }

  deleteBranchOffice(serviceId, branchOfficeId): Observable<any> {
    return this.httpClient.delete("https://localhost:44367/api/BranchOffice/DeleteBranchOffice?serviceId=" + serviceId + "&branchOfficeId=" + branchOfficeId);
  }

  postMethodCreateBranchOffice(serviceId, branchOffice, uploadedImage: File): Observable<any> {
    this.formData = new FormData();

    this.formData.append('serviceId',serviceId);
    this.formData.append('address', branchOffice.address)
    this.formData.append('latitude', branchOffice.latitude);
    this.formData.append('longitude', branchOffice.longitude);
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post("https://localhost:44367/api/BranchOffice/PostBranchOffice", this.formData, { headers: headers });
  }
  
  editBranchOffice(serviceId, branchOfficeId, branchOffice, uploadedImage: File): Observable<any> {
    
    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    this.formData = new FormData();
    this.formData.append('id', branchOfficeId);
    this.formData.append('serviceId',serviceId);
    this.formData.append('address', branchOffice.Address)
    this.formData.append('latitude', branchOffice.latitude);
    this.formData.append('longitude', branchOffice.longitude);
    this.formData.append('image', uploadedImage, uploadedImage.name);
    
    return this.httpClient.put("https://localhost:44367/api/BranchOffice/PutBranchOffice", this.formData, { headers: headers });
  }
}

