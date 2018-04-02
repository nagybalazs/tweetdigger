import { Injectable } from '@angular/core';
import { Channel } from '../classes/classes';
import { Tweet } from '../classes/tweet.class';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { HubConnection, TransportType } from '@aspnet/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class ChannelService {

    private _hub: HubConnection;
    get hub(): HubConnection {
        return this._hub;
    }

    private _tweets: Tweet[] = new Array<Tweet>();

    private _tweetReceived: BehaviorSubject<Tweet> = new BehaviorSubject<Tweet>(new Tweet());
    get tweetReceived(): Observable<Tweet> {
        return this._tweetReceived.share();
    }

    constructor(private http: HttpClient) { }

    public createHubConnection(): ChannelService {
        this._hub = new HubConnection('./tweets', { transport: TransportType.ServerSentEvents });
        return this;
    }

    public startHubConnection(): Promise<void> {
        return this._hub.start()
            .then(() => {
                console.log('hub started');
            })
            .catch(err => {
                console.log(err);
            });
    }

    public joinHubGroup(group: string) {
        this._hub.invoke('JoinGroup', group);
        this._hub.on('JoinGroup', (data: string) => {
            console.log(data);
        });
    }

    public startChannel() {
        this.hub.on('Send', (data: any) => {
            let tweet = Tweet.create(data);
            this._tweets.unshift(tweet);
            this._tweetReceived.next(tweet);
        });
    }

    public getChannels(): Observable<Channel[]> {
        return this.http.get<Channel[]>('/api/tweet/channels')
            .map(response => {
                let result: Channel[] = new Array<Channel>();
                response.forEach((channel, index: number) => {
                    let mapped: Channel = new Channel();
                    mapped.endpoint = channel.endpoint;
                    mapped.name = channel.name;
                    mapped.closed = false;
                    mapped.column = index;
                    if (channel.tweets) {
                        channel.tweets.forEach(tweet => {
                            let mappedTweet = Tweet.create(tweet);
                            mapped.tweets.push(mappedTweet);
                        });
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

    public getTweetsByType(type: string) {
        return this._tweets.filter(tweet => tweet.type == type);
    }

}