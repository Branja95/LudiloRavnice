import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class RatingService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  getMethodGetRating(ratingId : string){
    return this.httpClient.get("http://localhost:51680/api/Ratings/GetRating?id=" + ratingId);
  }

  putMethodEditRating(rating): Observable<any> {
    this.formData.append('Value', rating.Value)

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result = this.httpClient.put("http://localhost:51680/api/Ratings/PutRating?ratingId=" + rating.Id, this.formData, { headers: headers });

    this.formData = new FormData();

    return result;
  }

  postMethodCreateRating(rating):Observable<any>{
    return this.httpClient.post("http://localhost:51680/api/Ratings/PostRating", rating);
  }
}
