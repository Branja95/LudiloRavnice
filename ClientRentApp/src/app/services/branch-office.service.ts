import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { environment} from '../../environments/environment';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { BranchOffice } from '../models/branch-office.model';
import { env } from 'process';

@Injectable({
  providedIn: 'root'
})
export class BranchOfficeService {

  formData: FormData = new FormData();
  
  constructor(private httpClient: HttpClient) { }

  getBranchOffice(branchOfficeId): Observable<any> {
    return this.httpClient.get(environment.endpointBranchOfficeGetBranchOffice + branchOfficeId);
  }

  getBranchOffices(serviceId): Observable<any> {
    return this.httpClient.get(environment.endpointBranchOfficeGetBranchOffices + serviceId);
  }

  getVehicleServiceBranchOffices(vehicleId): Observable<any> {
    return this.httpClient.get(environment.endpointBranchOfficeGetVehicle + vehicleId);
  }

  deleteBranchOffice(serviceId, branchOfficeId): Observable<any> {
    return this.httpClient.delete(environment.endpointBranchOfficeDelete + serviceId + "&branchOfficeId=" + branchOfficeId);
  }

  postMethodCreateBranchOffice(serviceId, branchOffice, uploadedImage: File): Observable<any> {
    this.formData = new FormData();

    this.formData.append('serviceId',serviceId);
    this.formData.append('name', branchOffice.name)
    this.formData.append('address', branchOffice.address)
    this.formData.append('latitude', branchOffice.Latitude);
    this.formData.append('longitude', branchOffice.Longitude);
    this.formData.append('image', uploadedImage, uploadedImage.name);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post(environment.endpointBranchOfficeCreate, this.formData, { headers: headers });
  }
  
  editBranchOffice(serviceId, branchOfficeId, branchOffice, uploadedImage: File): Observable<any> {
    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')
    this.formData = new FormData();
    this.formData.append('id', branchOfficeId);
    this.formData.append('serviceId',serviceId);
    this.formData.append('name', branchOffice.Name)
    this.formData.append('address', branchOffice.Address)
    this.formData.append('latitude', branchOffice.Latitude);
    this.formData.append('longitude', branchOffice.Longitude);
    if(uploadedImage.name != null){
      this.formData.append('image', uploadedImage, uploadedImage.name);
    }
    
    return this.httpClient.put(environment.endpointBranchOfficeEdit, this.formData, { headers: headers });
  }
}

