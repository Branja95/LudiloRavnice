import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})

export class CommentService {
  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  getMethodGetComment(commentId : string){
    return this.httpClient.get(environment.endpointBookingGetComment + commentId);
  }

  getMethodUsername(commentId): Observable<any>{
    return this.httpClient.get(environment.endpointBookingCommentGetUserName + commentId);
  }

  postMethodCreateComment(comment): Observable<any>{
    this.formData = new FormData();

    this.formData.append('serviceId', comment.serviceId);
    this.formData.append('text', comment.text);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post(environment.endpointBookingCreateComment, this.formData, { headers: headers }); 
  }

  putMethodEditComment(comment): Observable<any> {
    this.formData = new FormData();

    this.formData.append('id', comment.Id);
    this.formData.append('text', comment.Text);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put(environment.endpointBookingEditComment, this.formData, { headers: headers });
  }
}