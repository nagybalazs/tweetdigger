import { Injectable } from '@angular/core';
import { Channel } from '../classes/classes';
import { Tweet } from '../classes/tweet.class';

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

    public faszom(): Tweet[] {
        return this.http.get('');
    }

}