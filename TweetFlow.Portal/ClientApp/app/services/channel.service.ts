import { Injectable } from '@angular/core';
import { Channel } from '../classes/classes';
import { Tweet } from '../classes/tweet.class';
import { HttpClient } from '@angular/common/http';
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

    public getCachedTweets(): Observable<Tweet[]> {
        return this.http.get<Tweet[]>('/api/tweet/cachedtweets');
    }

}