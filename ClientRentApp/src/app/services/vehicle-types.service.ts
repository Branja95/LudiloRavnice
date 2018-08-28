import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class VehicleTypesService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  getMethodVehicleType(vehicleTypeId): Observable<any>{
    return this.httpClient.get("http://localhost:51680/api/VehicleTypes/GetVehicleType?vehicleTypeId=" + vehicleTypeId);
  }

  getMethodVehicleTypes(): Observable<any>{
    return this.httpClient.get("http://localhost:51680/api/VehicleTypes/GetVehicleTypes");
  }

  putMethodVehicleTypes(vehicleType): Observable<any>{
    return this.httpClient.put("http://localhost:51680/api/VehicleTypes/PutVehicleType", vehicleType);
  }

  postMethodVehicleTypes(vehicleTypeName): Observable<any>{
    return this.httpClient.post("http://localhost:51680/api/VehicleTypes/PostVehicleType", vehicleTypeName);
  }
}
