var strokes = [];
var fills = [];
var symbols = [];
var timelines = [];
var namedTimelines = {};

var definitions = [];
var gradientStart = 0;

var boxSize = 25;

function createContext(width, height) 
{
	var canvas = document.createElement('canvas');
	canvas.width = width;
	canvas.height = height;	
	document.body.appendChild(canvas);
	
	return canvas;
}
function transformObject(obj, instance, offsetX, offsetY) 
{	
	var orgTxt = offsetX + "px " + offsetY + "px";
	obj.style["WebkitTransformOrigin"] = orgTxt;
	obj.style["msTransformOrigin"] = orgTxt;
	obj.style["OTransformOrigin"] = orgTxt;
	obj.style["MozTransformOrigin"] = orgTxt;
		
	var trans = "";	
	
	orgX = instance.x - offsetX;
	orgY = instance.y - offsetY;
	trans += "translate(" + orgX + "px," + orgY + "px)";
	
	if(instance.hasShear)
	{
		trans += "skewX(" + instance.shear + ") ";
	}	
	if(instance.hasRotation)
	{
		trans += "rotate(" + instance.rotation + "turn) ";
	}	
	//if(instance.hasScale)
	//{
	//	trans += "scale(" + instance.scaleX + "," + instance.scaleY + ") ";
	//}	

	obj.style["WebkitTransform"] = trans;
	obj.style["msTransform"] = trans;
	obj.style["OTransform"] = trans;	
	obj.style['MozTransform'] = trans;
}

function getColorString(value)
{
	var a = (value & 0xFF000000) >>> 24;
	var r = (value & 0xFF0000) >>> 16;
	var g = (value & 0x00FF00) >>> 8;
	var b = (value & 0x0000FF);
	var result = r + "," + g + "," + b;
	if(a < 255)
	{
		result = "rgba(" + result + "," + a + ")";
	}
	else
	{
		result = "rgb(" + result + ")";
	}
	return result;
}

function parseVex()
{
	for(var i = 0; i < data.strokes.length; i += 2)
	{
		var lineWidth = data.strokes[i];
		var col = getColorString(data.strokes[i + 1]);
		strokes.push([lineWidth, col]);
	}
	
	var cv = createContext(100,100);
	var g = cv.getContext("2d");
	
	for(var i = 0; i < data.fills.length; i++)
	{
		if(data.fills[i] instanceof Array)
		{
			// gradient
			// ["L",[-17.98,82.54,-79.42,-77.04],[4280796160,0,4291297159,0.39]],
			var gradKind = data.fills[i][0]; // "L" or "R"
			var tlbr = data.fills[i][1];
			var gradStops = data.fills[i][2];
			
			var grad = g.createLinearGradient(tlbr[0], tlbr[1], tlbr[2], tlbr[3]);
			for(var gs = 0; gs < gradStops.length; gs += 2)
			{
				var col = getColorString(gradStops[gs]);
				grad.addColorStop(gradStops[gs + 1], col);
			}    
			fills.push(grad);
		}
		else
		{
			// solid fill
			var col = getColorString(data.fills[i]);
			fills.push(col);
			gradientStart = i + 1;
		}
	}
	
	for(var i = 0; i < data.symbols.length; i++)
	{
		// "id":2,"name":"","bounds":[-66,-45.95,73,95.2],"shapes":[[][]]	
		// [2,0,"M-49.05,-7.85 C-51.2..."]
		var dsym = data.symbols[i];	
		
		var symbol = {};
		symbol.isTimeline = false;
		symbol.id = dsym.id;
		symbol.bounds = dsym.bounds;
		
		symbol.shapes = [];
		for(var sh = 0; sh < dsym.shapes.length; sh++)
		{
			var dshape = dsym.shapes[sh];
			var shape = {};
			shape.strokeIndex = dshape[0];
			shape.fillIndex = dshape[1];
			shape.segs = [];
			var segs = dshape[2].split(" ");
			for(var seg = 0; seg < segs.length; seg++)
			{
				var s = segs[seg];
				var nums = s.substring(1).split(",");
				switch(s[0])
				{
					case 'M':
						shape.segs.push([0, parseFloat(nums[0]), parseFloat(nums[1])]); 
						break;
					case 'L':
						shape.segs.push([1, parseFloat(nums[0]), parseFloat(nums[1])]); 
						break;
					case 'Q':
						shape.segs.push([2, parseFloat(nums[0]), parseFloat(nums[1]), parseFloat(nums[2]), parseFloat(nums[3])]); 
						break;
					case 'C':
						shape.segs.push([3, parseFloat(nums[0]), parseFloat(nums[1]), parseFloat(nums[2]), parseFloat(nums[3]), parseFloat(nums[4]), parseFloat(nums[5])]); 
						break;
				}
			}
			symbol.shapes.push(shape);
		}			
		
		symbols[symbol.id] = symbol;	
		definitions[symbol.id] = symbol;	
	}
	
	
	// timelines
	// "id":10,"name":"tripod","bounds":[95,31,120,117],
	// "instances":[[9,[96.85,86.2]], [11,[227,103.98],[0.9721,0.2348,-0.2348,0.9721]] ]		
	for(var i = 0; i < data.timelines.length; i++)
	{
		var dtl = data.timelines[i];
		var tl = {};
		tl.isTimeline = true;
		tl.id = dtl.id;
		tl.name = dtl.name;
		tl.bounds = dtl.bounds;
		tl.instances = [];
		for(var j = 0; j < dtl.instances.length; j++)
		{
			// [id,[x,y],[scaleX, scaleY, rotation*, skew*], "name"]
			var dinst = dtl.instances[j];
			var inst = {};
			
			inst.hasScale = false;
			inst.hasRotation = false;
			inst.hasShear = false;
			inst.scaleX = 1;
			inst.scaleY = 1;
			inst.rotation = 0;
			inst.shear = 0;
			
			inst.defId = dinst[0];
			
			inst.x = dinst[1][0];
			inst.y = dinst[1][1];
			
			if(dinst.length > 2 && typeof(dinst[2]) != "string")
			{
				var mxComp = dinst[2];
				
				inst.scaleX = mxComp[0];
				inst.scaleY = mxComp[1];
				inst.hasScale = true;
				
				if(mxComp.length > 2)
				{
					inst.rotation = mxComp[2];
					inst.hasRotation = true;
				}
				
				if(mxComp.length > 3)
				{
					inst.shear = mxComp[3];
					inst.hasShear = true;
				}
			}			

			if(dinst.length > 3)
			{
				inst.name = dinst[3];
			}
			else if(dinst.length > 2 && typeof(dinst[2]) == "string")
			{
				inst.name = dinst[2];
			}
			else
			{
				inst.name = "";
			}
			
			tl.instances.push(inst);
		}
		
		timelines[tl.id] = tl;	
		definitions[tl.id] = tl;		
		if(tl.name != null)
		{
			namedTimelines[tl.name] = tl;
		}	
	}
}

function drawColorTables()
{	
	for(var i = 0; i < strokes.length; i++)
	{
		var cv = createContext(boxSize, boxSize);
		var g = cv.getContext("2d");
		g.fillStyle =  fills[0];
		g.lineWidth = strokes[i][0];
		g.strokeStyle =  strokes[i][1];
		g.strokeRect(0, 0, boxSize, boxSize);		
		transformObject(cv, {x:(boxSize + 2) * i, y:40}, 0, 0);
	}
	
	for(var i = 0; i < fills.length; i++)
	{
		if(i < gradientStart)
		{
			var cv = createContext(boxSize, boxSize);
			var g = cv.getContext("2d");			
			g.fillStyle = fills[i];
			g.fillRect(0, 0, boxSize, boxSize);			
			transformObject(cv,{x:(boxSize + 2) * i, y:70}, 0, 0);
		}
		else
		{
			var cv = createContext(boxSize, boxSize);
			var g = cv.getContext("2d");
			g.fillStyle = fills[i];
			g.fillRect(0, 0, boxSize, boxSize);
			transformObject(cv, {x:(boxSize + 2) * (i - gradientStart + 1), y:100}, 0, 0);
		}
	}
}
function drawSymbol(id, metrics)
{	
	var symbol = symbols[id];
	if(symbol != null)
	{
		var bnds = symbol.bounds;
		var offsetX = -bnds[0] * metrics.scaleX;
		var offsetY = -bnds[1] * metrics.scaleY;
		
		var cv = createContext(bnds[2] * metrics.scaleX, bnds[3] * metrics.scaleY);
		//transformObject(cv, metrics, offsetX, offsetY);
		transformObject(cv, metrics, offsetX, offsetY);
		
		var g = cv.getContext("2d");
		g.translate(offsetX, offsetY);
		if(metrics.hasScale)
		{
			g.scale(metrics.scaleX, metrics.scaleY);
		}
		
		for(var i = 0; i < symbol.shapes.length; i++)
		{
			var shape = symbol.shapes[i];
			g.fillStyle = fills[shape.fillIndex];
			g.lineWidth = strokes[shape.strokeIndex][0];
			g.strokeStyle = strokes[shape.strokeIndex][1];
			
			g.beginPath();
			for(var j = 0; j < shape.segs.length; j++)
			{
				var seg = shape.segs[j];
				switch(seg[0])
				{
					case 0:	
						g.moveTo(seg[1], seg[2]);		
						break;
					case 1:	
						g.lineTo(seg[1], seg[2]);	
						break;
					case 2:	
						g.quadraticCurveTo(seg[1], seg[2], seg[3], seg[4]);
						break;
					case 3:	
						g.bezierCurveTo(seg[1], seg[2], seg[3], seg[4], seg[5], seg[6]);
						break;
				}
			}
			//g.closePath();
			
			if(shape.fillIndex > 0)
			{
				g.fill();
			}
			
			if(shape.strokeIndex > 0)
			{
				g.stroke();
			}
			
		}
		
	}
}

function drawTimeline(index, parent)
{	
	//Timeline: id,name,bounds,instances
	//		instance:defId,x,y,scaleX,scaleY,rotation,shear,name
	
	var tl = timelines[index];
	if(tl != null)
	{
		var bnds = tl.bounds;
		var offsetX = -bnds[0];
		var offsetY = -bnds[1];
		
		for(var i = 0; i < tl.instances.length; i++)
		{
			var inst = tl.instances[i];
			var def = definitions[inst.defId];
			if(def.isTimeline)
			{
				drawTimeline(def.id, inst)
			}
			else
			{
				drawSymbol(inst.defId, parent);
			}
		}
	}
}

     