#!/usr/bin/env python3
"""Original deterministic eight-bar music; Python standard library only.
No recordings, samples, external tunes, downloads, plugins or runtime synthesis.
All event timings, melodies, voicings and synthesizers are authored in this file.
Usage: python3 compose-original-music.py --game rising --output path/track.wav
Without --game, renders four audition loops to --output (a directory).
"""
import argparse, array, hashlib, json, math, random, struct, wave
from pathlib import Path
RATE=22050
TAU=math.tau
SCORES={
 'rising':dict(title='A Path Through the Air',bpm=84,seed=11011,lead='flute',keys='D major',
  chords=[[50,57,61,66],[43,55,59,62],[47,54,59,62],[45,57,61,64],[43,55,59,64],[50,57,61,66],[40,55,59,64],[45,57,62,64]],
  melody=[[(.5,74,1.0),(2,76,.6),(3,78,.7)],[(0,81,1.25),(2.5,78,1)],[(.5,78,.8),(1.75,76,.7),(3,74,.7)],[(1,73,1),(3,76,.75)],[(0,79,1.25),(2,78,.75),(3,76,.65)],[(.5,74,1.25),(2.5,69,.8)],[(0,71,1.2),(2,74,.8)],[(.5,73,1.25),(2.75,69,.8)]]),
 'floor':dict(title='Windows at Golden Hour',bpm=80,seed=11012,lead='rhodes',keys='C major / A minor',
  chords=[[48,55,59,64],[45,55,60,64],[41,53,57,60],[43,55,59,64],[48,55,59,64],[40,55,59,62],[41,53,57,64],[43,55,60,64]],
  melody=[[(.75,64,.7),(2.5,67,.9)],[(.5,69,1.3),(3,67,.55)],[(1,65,1.2),(3,64,.55)],[(.75,62,.7),(2.75,64,.8)],[(.5,67,1.2),(2.75,71,.6)],[(1,69,.75),(2.5,67,1)],[(.75,65,1.2),(3,64,.6)],[(.75,62,1),(2.5,59,1)]]),
 'drop':dict(title='Brass Gardens Above the Canal',bpm=72,seed=11013,lead='mallet',keys='E-flat major',
  chords=[[51,58,62,65],[48,55,58,63],[44,56,60,63],[46,58,60,65],[43,55,58,63],[41,56,60,63],[44,56,60,67],[46,58,63,65]],
  melody=[[(0,75,1.2),(2.5,77,.8)],[(.75,79,1.1),(3,77,.7)],[(.25,75,1.4),(2.75,72,.8)],[(1,70,1.1),(3,74,.65)],[(.5,75,1.25),(2.5,79,.8)],[(.75,80,1.25),(3,79,.65)],[(.25,77,1.1),(2,75,1)],[(.75,74,1.1),(2.75,70,.7)]]),
 'repair':dict(title='The Workshop Opens',bpm=92,seed=11014,lead='guitar',keys='G major',
  chords=[[43,55,59,62],[40,55,59,62],[48,55,59,64],[38,54,57,60],[43,55,59,62],[47,54,57,62],[48,55,60,64],[38,54,57,59]],
  melody=[[(.5,67,.7),(2.25,69,.5),(3,71,.7)],[(.5,74,1),(2.75,71,.6)],[(.5,72,.75),(2,71,.6),(3,67,.6)],[(1,66,1),(3,69,.65)],[(.5,71,.7),(2.25,74,.7)],[(.75,71,1.15),(2.75,69,.6)],[(.5,67,.8),(2.25,64,.8)],[(.75,66,.85),(2.75,62,.85)]])
}
def hz(note): return 440*2**((note-69)/12)
def render(game):
 score=SCORES[game]; beat=60/score['bpm']; frames=round(beat*32*RATE)
 left=array.array('d',[0])*frames;right=array.array('d',[0])*frames;events=[]
 rng=random.Random(score['seed'])
 def add(beat_at,note,beats,kind,amp,pan=0):
  start=round(beat_at*beat*RATE);gate=beats*beat
  release={'pad':1.25,'flute':.24,'piano':1.25,'rhodes':1.1,'mallet':1.1,'guitar':.85,'bass':.40}.get(kind,.14)
  length=gate+release; count=round(length*RATE); freq=hz(note) if note else 1
  gain_l=math.sqrt((1-pan)*.5);gain_r=math.sqrt((1+pan)*.5)
  events.append(dict(beat=round(beat_at,4),pitch=note,duration=beats,instrument=kind,gain=amp,pan=pan))
  # Acoustic string uses a filtered, damped circular delay excited by original noise.
  if kind=='guitar':
   period=max(2,round(RATE/freq));string=[rng.uniform(-1,1) for _ in range(period)]
   average=sum(string)/period;string=[(x-average)*.75 for x in string];pointer=0
  smoothed_noise=0
  for i in range(count):
   t=i/RATE;phase=TAU*freq*t
   if kind=='pad':
    attack=min(1,t/.48);release_gate=max(0,min(1,(length-t)/release))
    envelope=math.sin(attack*math.pi/2)**2*math.sin(release_gate*math.pi/2)**2
    value=(math.sin(phase)+.23*math.sin(phase*2+.04)+.09*math.sin(phase*3))*.69*envelope
   elif kind=='flute':
    attack=min(1,t/.095);end=max(0,min(1,(length-t)/release));envelope=math.sin(attack*math.pi/2)**2*end**2
    vibrato=.014*math.sin(TAU*4.6*t)*min(1,t/.4)
    smoothed_noise=smoothed_noise*.90+rng.uniform(-1,1)*.10
    value=(math.sin(phase+vibrato)+.15*math.sin(2*phase)+.045*math.sin(3*phase)+.035*smoothed_noise)*envelope
   elif kind in ('piano','rhodes','mallet'):
    attack=1-math.exp(-t*170);tail=max(0,min(1,(length-t)/.15))**2
    if kind=='piano':
     value=(math.sin(phase)*math.exp(-t*2.1)+.27*math.sin(phase*2.003)*math.exp(-t*3.1)+.12*math.sin(phase*3.008)*math.exp(-t*4.4)+.04*math.sin(phase*4.016)*math.exp(-t*6.0))*attack*tail
    elif kind=='rhodes':
     value=(math.sin(phase+.9*math.sin(phase*2)*math.exp(-t*3))*math.exp(-t*2.0)+.12*math.sin(phase*3.001)*math.exp(-t*4.2))*attack*tail
    else:
     value=(math.sin(phase)*math.exp(-t*2.7)+.30*math.sin(phase*3.97)*math.exp(-t*7.5)+.075*math.sin(phase*7.91)*math.exp(-t*12))*attack*tail
   elif kind=='guitar':
    value=string[pointer];next_pointer=(pointer+1)%period
    string[pointer]=(string[pointer]+string[next_pointer])*.4975;pointer=next_pointer
    value*=min(1,t/.004)*max(0,min(1,(length-t)/.12))**2
   elif kind=='bass':
    envelope=min(1,t/.03)*math.exp(-t*1.9)*max(0,min(1,(length-t)/.18))**2
    value=(math.sin(phase)+.15*math.sin(phase*2))*envelope
   elif kind=='brush':
    smoothed_noise=.60*smoothed_noise+.40*rng.uniform(-1,1)
    value=smoothed_noise*min(1,t/.006)*math.exp(-t*35)*max(0,(length-t)/length)
   elif kind=='kick':
    value=math.sin(TAU*(54*t+1.2*(1-math.exp(-t*24))))*min(1,t/.008)*math.exp(-t*19)*max(0,(length-t)/length)
   else:raise ValueError(kind)
   idx=(start+i)%frames;value*=amp;left[idx]+=value*gain_l;right[idx]+=value*gain_r
 # Eight bars: changing voiced chords, responding original melody, and breathing arrangements.
 for bar,chord in enumerate(score['chords']):
  base=bar*4;gentle=.82 if bar in (3,7) else 1
  if game=='rising':
   for n in chord[1:]:add(base,n,3.1,'pad',.027*gentle,(-.55 if n%2 else .55))
   for b,n in zip([0,.75,1.5,2.5,3.25],[chord[1],chord[2],chord[3],chord[2],chord[1]+12]):add(base+b,n,.55,'piano',.14*gentle,(-.32 if b<2 else .28))
   add(base,chord[0],2.2,'bass',.12)
   if bar%2==0:add(base+2.75,chord[2]+12,.3,'mallet',.026,.6)
  elif game=='floor':
   for b in [0,1.5,2.75]:
    for j,n in enumerate(chord[1:]):add(base+b+j*.035,n,.65,'rhodes',.085*gentle,-.3+j*.3)
   add(base,chord[0]-12,1.0,'bass',.16);add(base+2.5,chord[0],.8,'bass',.09)
   for b in [0,2.5]:add(base+b,None,.17,'kick',.045)
   for b in [.5,1.5,2.5,3.5]:add(base+b,None,.16,'brush',.028,.30)
  elif game=='drop':
   for j,n in enumerate(chord[1:]):add(base-.18+j*.035,n,3.65,'pad',.037*gentle,-.55+j*.55)
   for b,n in zip([.5,1.5,2.75],[chord[1]+12,chord[2],chord[3]]):add(base+b,n,.9,'mallet',.13*gentle,-.25 if b<2 else .3)
   add(base,chord[0],2.9,'bass',.10)
  else:
   for b in [0,1.75,2.75]:
    for j,n in enumerate(chord[1:]):add(base+b+j*.038,n,.7,'guitar',.16*gentle,-.32+j*.32)
   add(base,chord[0],.8,'bass',.19);add(base+2,chord[0]+7,.65,'bass',.13)
   for b in [.65,1.5,2.65,3.5]:add(base+b,None,.18,'brush',.036,.45)
   for b in [0,2]:add(base+b,None,.17,'kick',.027)
  for b,n,d in score['melody'][bar]:add(base+b,n,d,score['lead'],(.095 if game=='rising' else .16 if game=='repair' else .12)*gentle,.10)
 # Short circular stereo room echoes; delayed tails wrap seamlessly into the next cycle.
 dry_l=array.array('d',left);dry_r=array.array('d',right)
 for seconds,amount,cross in [(.113,.13,False),(.229,.095,True),(.371,.055,True)]:
  offset=round(seconds*RATE)
  for i in range(frames):
   idx=(i+offset)%frames
   left[idx]+=(dry_r[i] if cross else dry_l[i])*amount
   right[idx]+=(dry_l[i] if cross else dry_r[i])*amount
 # Normalize RMS instead of limiting/clipping peaks. Preserve natural phrasing and headroom.
 dc_l=sum(left)/frames;dc_r=sum(right)/frames
 for i in range(frames):left[i]-=dc_l;right[i]-=dc_r
 rms=math.sqrt((sum(x*x for x in left)+sum(x*x for x in right))/(frames*2));peak=max(max(abs(x) for x in left),max(abs(x) for x in right));gain=min(.112/rms,.70/peak)
 pcm=array.array('h');energy=[]
 for i in range(frames):pcm.append(round(left[i]*gain*32767));pcm.append(round(right[i]*gain*32767))
 if struct.pack('=H',1)!=struct.pack('<H',1):pcm.byteswap()
 values=lambda channel:[pcm[i*2+channel]/32768 for i in range(frames)]
 ls=values(0);rs=values(1)
 for start in range(0,frames-RATE//2,RATE//2):energy.append(math.sqrt(sum(x*x for x in ls[start:start+RATE//2])/(RATE//2)))
 report=dict(title=score['title'],game=game,bpm=score['bpm'],bars=8,meter='4/4',tonality=score['keys'],sample_rate=RATE,channels=2,frames=frames,duration=frames/RATE,pcm_bytes=len(pcm)*2,
  peak=max(max(abs(x) for x in ls),max(abs(x) for x in rs)),rms=math.sqrt((sum(x*x for x in ls)+sum(x*x for x in rs))/(frames*2)),dc=max(abs(sum(ls)/frames),abs(sum(rs)/frames)),
  seam=max(abs(ls[0]-ls[-1]),abs(rs[0]-rs[-1])),seam_slope=max(abs((ls[1]-ls[0])-(ls[0]-ls[-1])),abs((rs[1]-rs[0])-(rs[0]-rs[-1]))),window_rms_min=min(energy),window_rms_max=max(energy),events=len(events),original_score=events)
 assert .2<=report['peak']<=.82 and .035<=report['rms']<=.18,report
 assert report['seam']<.015 and report['seam_slope']<.015 and report['dc']<.001,report
 assert report['window_rms_min']>.008 and report['window_rms_max']-report['window_rms_min']>.015,report
 return pcm,report

def write(game,path):
 pcm,report=render(game);path=Path(path);path.parent.mkdir(parents=True,exist_ok=True)
 with wave.open(str(path),'wb') as output:output.setnchannels(2);output.setsampwidth(2);output.setframerate(RATE);output.writeframes(pcm.tobytes())
 report['sha256']=hashlib.sha256(path.read_bytes()).hexdigest();report['composer_sha256']=hashlib.sha256(Path(__file__).read_bytes()).hexdigest()
 path.with_suffix('.music.json').write_text(json.dumps(report,indent=2)+'\n')
 print(game,json.dumps({k:v for k,v in report.items() if k!='original_score'}),flush=True)
 return report
if __name__=='__main__':
 parser=argparse.ArgumentParser();parser.add_argument('--game',choices=list(SCORES));parser.add_argument('--output',type=Path,required=True);args=parser.parse_args()
 if args.game:write(args.game,args.output)
 else:
  reports=[write(game,args.output/(game+'.wav')) for game in SCORES]
  assert len({r['sha256'] for r in reports})==4,'each game must retain its own composition'
  (args.output/'music-audit.json').write_text(json.dumps(reports,indent=2)+'\n')
