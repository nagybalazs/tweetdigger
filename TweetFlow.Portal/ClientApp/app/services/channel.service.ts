import { Injectable } from '@angular/core';
import { Channel } from '../classes/classes';
import { Tweet } from '../classes/tweet.class';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class ChannelService {

    constructor(private http: HttpClient) { }

    public getChannels(): Observable<Channel[]> {
        return this.http.get<string[]>('/api/tweet/channels')
            .map(response => {
                let result: Channel[] = new Array<Channel>();
                response.forEach(channel => {
                    let mapped: Channel = {
                        endpoint: channel,
                        name: `#${channel}`,
                        closed: false
                    };
                    result.push(mapped);
                });
                return result;
            });
    }

    public getCachedTweets(channel: string): Observable<Tweet[]> {
        let params = new HttpParams().set('channel', channel);
        return this.http.get<Tweet[]>('/api/tweet/cachedtweets', { params: params });
    }

}