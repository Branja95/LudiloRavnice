import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class VehicleTypesService {

  constructor(private httpClient: HttpClient) { }

  getMethodVehicleType(vehicleTypeId): Observable<any>{
    return this.httpClient.get("https://localhost:44367/api/VehicleType/GetVehicleType?vehicleTypeId=" + vehicleTypeId);
  }

  getMethodVehicleTypes(): Observable<any>{
    return this.httpClient.get("https://localhost:44367/api/VehicleType/GetVehicleTypes");
  }

  putMethodVehicleTypes(vehicleType): Observable<any>{
    return this.httpClient.put("https://localhost:44367/api/VehicleType/PutVehicleType", vehicleType);
  }

  postMethodVehicleTypes(vehicleTypeName): Observable<any>{
    return this.httpClient.post("https://localhost:44367/api/VehicleType/PostVehicleType", vehicleTypeName);
  }
  
}
