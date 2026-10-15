#!/usr/bin/env python3
"""Synthetic release-validator plumbing fixtures; never evidence of real art approval."""
from pathlib import Path
import copy,hashlib,json,runpy,struct,tempfile,zlib

api=runpy.run_path(str(Path(__file__).with_name('verify-visual-release.py')))
validate=api['validate_store_icon'];fingerprint=api['fingerprint']
checks=0
with tempfile.TemporaryDirectory(prefix='synthetic-icon-gate-')as temp:
 root=Path(temp)
 for d in ('Assets','Packages','ProjectSettings','docs/QA','Native'): (root/d).mkdir(parents=True,exist_ok=True)
 def png(width=1024,height=1024,kind=2):
  def chunk(tag,data):return struct.pack('>I',len(data))+tag+data+struct.pack('>I',zlib.crc32(tag+data)&0xffffffff)
  channels=4 if kind==6 else 3;row=b'\0'+bytes([90]*channels)*width
  return b'\x89PNG\r\n\x1a\n'+chunk(b'IHDR',struct.pack('>IIBBBBB',width,height,8,kind,0,0,0))+chunk(b'IDAT',zlib.compress(row*height))+chunk(b'IEND',b'')
 for name in ('Assets/icon.png','Native/icon.png','Native/frame.png'):(root/name).write_bytes(png())
 (root/'Assets/icon.png.meta').write_text('fileFormatVersion: 2\nguid: 0123456789abcdef0123456789abcdef\n')
 settings=root/'ProjectSettings/ProjectSettings.asset';settings.write_text('  m_BuildTargetIcons:\n  - m_BuildTarget: \n    m_Icons:\n    - m_Icon: {guid: 0123456789abcdef0123456789abcdef}\n  m_BuildTargetPlatformIcons: []\n')
 def evidence(name,origin):return {'path':name,'origin':origin,'sha256':hashlib.sha256((root/name).read_bytes()).hexdigest()}
 base={'status':'passed','sourceSha256':fingerprint(root),'reviewer':'SYNTHETIC TEST ONLY','reviewedAt':'test','decision':'Synthetic validator plumbing, not actual metadata approval','variantConsistencyChecked':True,'evidence':{'sourceIcon':evidence('Assets/icon.png','project-icon'),'nativeIcon':evidence('Native/icon.png','native-build-icon'),'gameplay':[evidence('Native/frame.png','Unity-render')]}}
 base['evidence']['nativeIcon']['sourceSha256']=base['sourceSha256']
 path=root/'docs/QA/store-icon-review.json'
 def test(label,doc,accept=False):
  global checks
  path.write_text(json.dumps(doc))
  try:validate(root)
  except(ValueError,OSError,KeyError,TypeError):
   if accept:raise AssertionError('Rejected valid synthetic fixture '+label)
  else:
   if not accept:raise AssertionError('Accepted invalid icon metadata '+label)
  checks+=1
 test('valid fixture',base,True)
 for key,value in (('status','pending'),('status','failed'),('sourceSha256','stale'),('reviewer',''),('decision',''),('variantConsistencyChecked',False)):
  doc=copy.deepcopy(base);doc[key]=value;test(key+' '+str(value),doc)
 for field in ('sourceIcon','nativeIcon'):
  doc=copy.deepcopy(base);doc['evidence'][field]['sha256']='bad';test(field+' changed hash',doc)
 doc=copy.deepcopy(base);doc['evidence']['nativeIcon']=None;test('missing native export',doc)
 doc=copy.deepcopy(base);doc['evidence']['nativeIcon']['sourceSha256']='other';test('native export from other source',doc)
 doc=copy.deepcopy(base);doc['evidence']['gameplay'][0]['origin']='generated-mockup';test('mockup is not gameplay',doc)
 doc=copy.deepcopy(base);doc['evidence']['gameplay']=[];test('no gameplay',doc)
 for label,data in (('small native icon',png(512,512)),('RGBA native icon',png(kind=6)),('non-square native icon',png(1024,512))):
  (root/'Native/icon.png').write_bytes(data);doc=copy.deepcopy(base);doc['evidence']['nativeIcon']['sha256']=hashlib.sha256(data).hexdigest();test(label,doc)
 (root/'Native/icon.png').write_bytes(png())
 # Header-shaped or CRC-damaged files must not be accepted as actual image evidence.
 for label,data in [('truncated header',png()[:26]),('CRC damaged PNG',png()[:-1]+bytes([png()[-1]^1]))]:
  (root/'Native/icon.png').write_bytes(data);doc=copy.deepcopy(base);doc['evidence']['nativeIcon']['sha256']=hashlib.sha256(data).hexdigest();test(label,doc)
 (root/'Native/icon.png').write_bytes(png())
 # A native iPhone override must not silently supersede the reviewed default.
 for label,guid,accept in [('matching iPhone override','0123456789abcdef0123456789abcdef',True),('conflicting iPhone override','fedcba9876543210fedcba9876543210',False)]:
  settings.write_text('  m_BuildTargetIcons:\n  - m_BuildTarget: \n    m_Icons:\n    - m_Icon: {guid: 0123456789abcdef0123456789abcdef}\n  m_BuildTargetPlatformIcons:\n  - m_BuildTarget: iPhone\n    m_Icons:\n    - m_Textures:\n      - {fileID: 2800000, guid: '+guid+', type: 3}\n  otherSetting: 0\n');doc=copy.deepcopy(base);doc['sourceSha256']=fingerprint(root);doc['evidence']['nativeIcon']['sourceSha256']=doc['sourceSha256'];test(label,doc,accept)
 settings.write_text('  m_BuildTargetIcons:\n  - m_BuildTarget: \n    m_Icons:\n    - m_Icon: {guid: fedcba9876543210fedcba9876543210}\n  m_BuildTargetPlatformIcons: []\n');doc=copy.deepcopy(base);doc['sourceSha256']=fingerprint(root);doc['evidence']['nativeIcon']['sourceSha256']=doc['sourceSha256'];test('reviewed PNG is not assigned icon',doc)
 print('STORE_ICON_GUARD_PASS',checks,'synthetic integrity/assignment cases; not a real icon approval')
