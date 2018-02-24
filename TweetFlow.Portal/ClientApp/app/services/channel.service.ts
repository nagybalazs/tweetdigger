import { Injectable } from '@angular/core';
import { Channel } from '../classes/classes';
import { Tweet } from '../classes/tweet.class';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class ChannelService {

    constructor(private http: HttpClient) { }

    public static getChannels(): Channel[] {
        return [
            { endpoint: 'bitcoin', name: '#bitcoin' },
            { endpoint: 'ethereum', name: '#ethereum' },
            { endpoint: 'ripple', name: '#ripple' },
            { endpoint: 'litecoin', name: '#litecoin' }
        ];
    }

    public getCachedTweets(channel: string): Observable<Tweet[]> {
        let params = new HttpParams().set('channel', channel);
        return this.http.get<Tweet[]>('/api/tweet/cachedtweets', { params: params });
    }

}