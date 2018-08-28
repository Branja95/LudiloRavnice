import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class CommentService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  getMethodGetComment(commentId : string){
    return this.httpClient.get("http://localhost:51680/api/Comments/GetComment?id=" + commentId);
  }

  getMethodGetUserName(commentId : string){
    return this.httpClient.get("http://localhost:51680/api/Comments/GetUserName?commentId=" + commentId);
  }

  putMethodEditComment(comment): Observable<any> {
    this.formData.append('Text', comment.Text)

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result = this.httpClient.put("http://localhost:51680/api/Comments/PutComment?commentId=" + comment.Id, this.formData, { headers: headers });

    this.formData = new FormData();

    return result;
  }

  postMethodCreateComment(comment): Observable<any>{
    this.formData.append('serviceId',comment.serviceId);
    this.formData.append('text', comment.text);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result =  this.httpClient.post("http://localhost:51680/api/Comments/PostComment", this.formData, { headers: headers }); 
    
    this.formData = new FormData();

    return result;
  }

  getMethodUsername(commentId): Observable<any>{
    console.log(commentId);
    let result = this.httpClient.get("http://localhost:51680/api/Comments/UserName?commentId=" + commentId); 
    console.log(result);
    return result;
  }

}
