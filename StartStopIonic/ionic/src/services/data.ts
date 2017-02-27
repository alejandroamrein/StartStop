import {Injectable} from '@angular/core';
import {Http, Response, Headers, RequestOptions /*, RequestOptionsArgs*/} from '@angular/http';
import 'rxjs/Rx';
import {Observable} from 'rxjs/Observable';
//import {Observer} from 'rxjs/Observer';
import 'rxjs/add/operator/share';
import 'rxjs/add/operator/map';

import {User} from '../model/user';
//import {SelectItem} from '../model/selectItem';
//import {Book} from '../model/book';

@Injectable()
export class DataService {
    private user: User;
    private serviceUrl: string = "http://startstopdataservice.azurewebsites.net";

    constructor(private http: Http) {
        var url = localStorage.getItem('url');
        console.log('URL: ' + url);
        if (url != null) {
            this.serviceUrl = url;
            console.log('Service URL: ' + this.serviceUrl);
        }
        var temp = localStorage.getItem('user');
        console.log('USER: ' + temp);
        if (temp != null) {
            this.user = JSON.parse(temp);
        }
    }

    logout() {
        this.user = null;
        localStorage.removeItem('user');
    }

    //login(email, password): Observable<User> {
    //    return this.http.get(`${this.serviceUrl}/Login/${email}/${password}`)
    //        .map((res: Response) => res.json())
    //        .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    //}

    login(name: string, password: string): Observable<Response> {
        return this.http.get(`${this.serviceUrl}/api/Login/${name}/${password}`);
    }

    getState(): Observable<Response> {
        return this.http.get(`${this.serviceUrl}/api/StartStop/State/${this.user.name}`);
    }

    getProjekte(): Observable<Response> {
        return this.http.get(`${this.serviceUrl}/api/Stamm/Projekte/${this.user.name}`);
    }

    getLohnkategorien(projektId: string): Observable<Response> {
        return this.http.get(`${this.serviceUrl}/api/Stamm/Lohnkategorien/${projektId}`);
    }

    getTarifkategorien(): Observable<Response> {
        return this.http.get(`${this.serviceUrl}/api/Stamm/Tarifkategorien`);
    }

    ping(url): Observable<Response> {
        console.log(`Ping at ${url}/api/Ping`);
        return this.http.get(`${url}/api/Ping`);
    }

    setCurrentUser(user: User) {
        this.user = user;
        localStorage.setItem('user', JSON.stringify(user));
    }

    getCurrentUser(): User {
        console.log('*** BooksService getCurrentUser called ' + this.user.name)
        return this.user;
    }

    setServiceUrl(url: string) {
        this.serviceUrl = url;
        localStorage.setItem('url', url);
    }

    getServiceUrl(): string {
        return this.serviceUrl;
    }

    getHistory(): Observable<Response> {
        return this.http.get(`${this.serviceUrl}/api/StartStop/History/${this.user.name}/7`);
    }

    //getProjekte(): Observable<SelectItem[]> {
    //    var url = `${this.serviceUrl}/api/Stamm/Projekte/${this.user.id}`;
    //    return this.http.get(url);
    //}

    //getBooks(filter): Observable<Book[]> {
    //    let url: string;
    //    if (filter === undefined || filter === '') {
    //        url = `${this.serviceUrl}/Books`;
    //    } else {
    //        url = `${this.serviceUrl}/Books/Search/${filter}`;
    //    }
    //    return this.http.get(url)
    //        .map((res: Response) => res.json())
    //        .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    //}

    //getBooks(filter) {
    //    if (filter === undefined || filter === '') {
    //        return this.http.get(`${this.serviceUrl}/Books`);
    //    } else {
    //        return this.http.get(`${this.serviceUrl}/Books/Search/${filter}`);
    //    }
    //}

    //getUserBooks(id) {
    //    return this.http.get(`http://booksdataservice.azurewebsites.net/api/Users/${id}/Books`);
    //}

    //getBookImages(id) {
    //    return this.http.get(`http://booksdataservice.azurewebsites.net/api/Books/${id}/Images`);
    //}

    //getBookOwner(id) {
    //    return this.http.get(`http://booksdataservice.azurewebsites.net/api/Books/${id}/Owner`);
    //}

    //getBookDetails(id) {
    //    return this.http.get(`http://booksdataservice.azurewebsites.net/api/Books/${id}`);
    //}

    getStartUrl(): string {
        return `${this.serviceUrl}/api/StartStop/State/${this.user.name}`;
    }

    start(projekt, tarifKategorie, lohnKategorie, text): Observable<Response> {
        var body = {
            Action: 'start',
            Projekt: projekt,
            Tarifkategorie: tarifKategorie,
            Lohnkategorie: lohnKategorie,
            Text: text
        };
        //var options: RequestOptions;
        //options = new RequestOptions();
        var headers: Headers;
        headers = new Headers();
        headers.append('Content-Type', 'application/json')
        //options.headers = headers;
        return this.http.put(`${this.serviceUrl}/api/StartStop/State/${this.user.name}`, JSON.stringify(body), { headers: headers });
    }

    startEx(projekt, tarifKategorie, lohnKategorie, text, datum): Observable<Response> {
        var body = {
            Action: 'start',
            Projekt: projekt,
            Tarifkategorie: tarifKategorie,
            Lohnkategorie: lohnKategorie,
            Text: text,
            Datum: datum
        };
        //var options: RequestOptions;
        //options = new RequestOptions();
        var headers: Headers;
        headers = new Headers();
        headers.append('Content-Type', 'application/json')
        //options.headers = headers;
        return this.http.put(`${this.serviceUrl}/api/StartStop/StateEx/${this.user.name}`, JSON.stringify(body), { headers: headers });
    }

    stop(): Observable<Response> {
        var body = {
            Action: 'stop'
        };
        var options: RequestOptions;
        options = new RequestOptions();
        var headers: Headers;
        headers = new Headers();
        headers.append('Content-Type', 'application/json')
        options.headers = headers;
        return this.http.put(`${this.serviceUrl}/api/StartStop/State/${this.user.name}`, JSON.stringify(body), { headers: headers });
    }

    cancel(): Observable<Response> {
        var body = {
            Action: 'cancel'
        };
        var options: RequestOptions;
        options = new RequestOptions();
        var headers: Headers;
        headers = new Headers();
        headers.append('Content-Type', 'application/json')
        options.headers = headers;
        return this.http.put(`${this.serviceUrl}/api/StartStop/State/${this.user.name}`, JSON.stringify(body), { headers: headers });
    }

    stopEx(datum): Observable<Response> {
        var body = {
            Action: 'stop',
            Datum: datum
        };
        var options: RequestOptions;
        options = new RequestOptions();
        var headers: Headers;
        headers = new Headers();
        headers.append('Content-Type', 'application/json')
        options.headers = headers;
        return this.http.put(`${this.serviceUrl}/api/StartStop/StateEx/${this.user.name}`, JSON.stringify(body), { headers: headers });
    }

    pause(): Observable<Response> {
        var body = {
            Action: 'pause'
        };
        var options: RequestOptions;
        options = new RequestOptions();
        var headers: Headers;
        headers = new Headers();
        headers.append('Content-Type', 'application/json')
        options.headers = headers;
        return this.http.put(`${this.serviceUrl}/api/StartStop/State/${this.user.name}`, JSON.stringify(body), { headers: headers });
    }

    resume(): Observable<Response> {
        var body = {
            Action: 'resume'
        };
        var options: RequestOptions;
        options = new RequestOptions();
        var headers: Headers;
        headers = new Headers();
        headers.append('Content-Type', 'application/json')
        options.headers = headers;
        return this.http.put(`${this.serviceUrl}/api/StartStop/State/${this.user.name}`, JSON.stringify(body), { headers: headers });
    }
}
