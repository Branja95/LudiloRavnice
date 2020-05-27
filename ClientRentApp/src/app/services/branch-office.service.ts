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
    return this.httpClient.get("https://localhost:5001/api/BranchOffices/GetBranchOffice?branchOfficeId=" + branchOfficeId);
  }

  getBranchOffices(serviceId): Observable<any> {
    return this.httpClient.get("https://localhost:5001/api/Services/GetBranchOffices?serviceId=" + serviceId);
  }

  getVehicleServiceBranchOffices(vehicleId): Observable<any> {
    return this.httpClient.get("https://localhost:5001/api/BranchOffices/GetVehicleBranchOffices?vehicleId=" + vehicleId);
  }

  deleteBranchOffice(serviceId, branchOfficeId): Observable<any> {
    return this.httpClient.delete("https://localhost:5001/api/BranchOffices/DeleteBranchOffice?serviceId=" + serviceId + "&branchOfficeId=" + branchOfficeId);
  }

  editBranchOffice(serviceId, branchOfficeId, branchOffice, uploadedImage: File): Observable<any> {
    this.formData = new FormData();

    this.formData.append('id', branchOfficeId);
    this.formData.append('address', branchOffice.Address)
    this.formData.append('latitude', branchOffice.latitude);
    this.formData.append('longitude', branchOffice.longitude);
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put("https://localhost:5001/api/BranchOffices/PutBranchOffice?serviceId=" + serviceId, this.formData, { headers: headers });
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

    return this.httpClient.post("https://localhost:5001/api/BranchOffices/PostBranchOffice",this.formData, { headers: headers });
  }
  
}

