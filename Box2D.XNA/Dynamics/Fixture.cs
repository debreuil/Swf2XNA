/*
* Box2D.XNA port of Box2D:
* Copyright (c) 2009 Brandon Furtwangler, Nathan Furtwangler
*
* Original source Box2D:
* Copyright (c) 2006-2009 Erin Catto http://www.gphysics.com 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Box2D.XNA
{
    /// This holds contact filtering data.
    public struct Filter
    {
        /// The collision category bits. Normally you would just set one bit.
        public UInt16 categoryBits;

        /// The collision mask bits. This states the categories that this
        /// Shape would accept for collision.
        public UInt16 maskBits;

        /// Collision groups allow a certain group of objects to never collide (negative)
        /// or always collide (positive). Zero means no collision group. Non-zero group
        /// filtering always wins against the mask bits.
        public Int16 groupIndex;
    };

    /// A fixture definition is used to create a fixture. This class defines an
    /// abstract fixture definition. You can reuse fixture definitions safely.
    public class FixtureDef
    {
	    /// The constructor sets the default fixture definition values.
        public FixtureDef()
	    {
		    shape = null;
		    userData = null;
		    friction = 0.2f;
		    restitution = 0.0f;
		    density = 0.0f;
		    filter.categoryBits = 0x0001;
		    filter.maskBits = 0xFFFF;
		    filter.groupIndex = 0;
		    isSensor = false;
	    }

	    /// The Shape, this must be set. The Shape will be cloned, so you
	    /// can create the Shape on the stack.
        public Shape shape;

	    /// Use this to store application specific fixture data.
        public object userData;

	    /// The friction coefficient, usually in the range [0,1].
        public float friction;

	    /// The restitution (elasticity) usually in the range [0,1].
        public float restitution;

	    /// The density, usually in kg/m^2.
        public float density;

	    /// A sensor Shape collects contact information but never generates a collision
	    /// response.
        public bool isSensor;

	    /// Contact filtering data.
        public Filter filter;
    };


    /// This proxy is used internally to connect fixtures to the broad-phase.
    public class FixtureProxy
    {
        public AABB aabb;
        public Fixture fixture;
        public int childIndex;
        public int proxyId;
    };

    /// A fixture is used to attach a Shape to a body for collision detection. A fixture
    /// inherits its transform from its parent. Fixtures hold additional non-geometric data
    /// such as friction, collision filters, etc.
    /// Fixtures are created via Body.CreateFixture.
    /// @warning you cannot reuse fixtures.
    public class Fixture
    {
	    /// Get the type of the child Shape. You can use this to down cast to the concrete Shape.
	    /// @return the Shape type.
	    public ShapeType ShapeType
        {
            get { return _shape.ShapeType; }
        }

	    /// Get the child Shape. You can modify the child Shape, however you should not change the
	    /// number of vertices because this will crash some collision caching mechanisms.
	    public Shape GetShape()
        {
            return _shape;
        }

	    /// Is this fixture a sensor (non-solid)?
	    /// @return the true if the Shape is a sensor.
	    public bool IsSensor()
        {
            return _isSensor;
        }

	    /// Set if this fixture is a sensor.
	    public void SetSensor(bool sensor)
        {
	        _isSensor = sensor;
        }

        /// Set the contact filtering data. This will not update contacts until the next time
        /// step when either parent body is active and awake.
	    public void SetFilterData(ref Filter filter)
        {
            _filter = filter;

	        if (_body == null)
	        {
		        return;
	        }

	        // Flag associated contacts for filtering.
	        ContactEdge edge = _body.GetContactList();
	        while (edge != null)
	        {
		        Contact contact = edge.Contact;
		        Fixture fixtureA = contact.GetFixtureA();
		        Fixture fixtureB = contact.GetFixtureB();
		        if (fixtureA == this || fixtureB == this)
		        {
			        contact.FlagForFiltering();
		        }

                edge = edge.Next;
	        }
        }

	    /// Get the contact filtering data.
	    public void GetFilterData(out Filter filter)
        {
            filter = _filter;
        }

	    /// Get the parent body of this fixture. This is null if the fixture is not attached.
	    /// @return the parent body.
	    public Body GetBody()
        {
            return _body;
        }

	    /// Get the next fixture in the parent body's fixture list.
	    /// @return the next Shape.
	    public Fixture GetNext()
        {
            return _next;
        }

	    /// Get the user data that was assigned in the fixture definition. Use this to
	    /// store your application specific data.
	    public object GetUserData()
        {
            return _userData;
        }

	    /// Set the user data. Use this to store your application specific data.
	    public void SetUserData(object data)
        {
            _userData = data;
        }

        public void SetDensity(float density)
        {
	        Debug.Assert(MathUtils.IsValid(density) && density >= 0.0f);
	        _density = density;
        }

        public float GetDensity()
        {
	        return _density;
        }


	    /// Test a point for containment in this fixture.
	    /// @param xf the Shape world transform.
	    /// @param p a point in world coordinates.
	    public bool TestPoint(Vector2 p)
        {
            Transform xf;
            _body.GetTransform(out xf);
            return _shape.TestPoint(ref xf, p);
        }

        /// Cast a ray against this Shape.
	    /// @param output the ray-cast results.
	    /// @param input the ray-cast input parameters.
        public bool RayCast(out RayCastOutput output, ref RayCastInput input, int childIndex)
        {
            Transform xf;
            _body.GetTransform(out xf);
            return _shape.RayCast(out output, ref input, ref xf, childIndex);
        }

	    /// Get the mass data for this fixture. The mass data is based on the density and
	    /// the Shape. The rotational inertia is about the Shape's origin.
	    public void GetMassData(out MassData massData)
        {
            _shape.ComputeMass(out massData, _density);
        }


	    /// Get the coefficient of friction.
	    public float GetFriction()
        {
            return _friction;
        }

	    /// Set the coefficient of friction.
	    public void SetFriction(float friction)
        {
            _friction = friction;
        }

	    /// Get the coefficient of restitution.
	    public float GetRestitution()
        {
            return _restitution;
        }

	    /// Set the coefficient of restitution.
	    public void SetRestitution(float restitution)
        {
            _restitution = restitution;
        }

        /// Get the fixture's AABB. This AABB may be enlarge and/or stale.
	    /// If you need a more accurate AABB, compute it using the Shape and
	    /// the body transform.
	    public void GetAABB(out AABB aabb, int childIndex)
        {
            Debug.Assert(0 <= childIndex && childIndex < _proxyCount);
            aabb = _proxies[childIndex].aabb;
        }

	    internal Fixture()
        {
            _userData = null;
	        _body = null;
	        _next = null;
	        _proxyId = BroadPhase.NullProxy;
	        _shape = null;
        }

	    // We need separation create/destroy functions from the constructor/destructor because
	    // the destructor cannot access the allocator or broad-phase (no destructor arguments allowed by C++).
	    internal void Create(Body body, FixtureDef def)
        {
            _userData = def.userData;
	        _friction = def.friction;
	        _restitution = def.restitution;

	        _body = body;
	        _next = null;

	        _filter = def.filter;

	        _isSensor = def.isSensor;

	        _shape = def.shape.Clone();

            // Reserve proxy space
	        int childCount = _shape.GetChildCount();
	        _proxies = new FixtureProxy[childCount];
	        for (int i = 0; i < childCount; ++i)
	        {
                _proxies[i] = new FixtureProxy();
		        _proxies[i].fixture = null;
		        _proxies[i].proxyId = BroadPhase.NullProxy;
	        }
	        _proxyCount = 0;

            _density = def.density;
        }

	    internal void Destroy()
        {
            // The proxies must be destroyed before calling this.
            Debug.Assert(_proxyCount == 0);

            // Free the proxy array.
            _proxies = null;

            _shape = null;
        }

        // These support body activation/deactivation.
	    internal void CreateProxies(BroadPhase broadPhase, ref Transform xf)
        {
            Debug.Assert(_proxyCount == 0);

            // Create proxies in the broad-phase.
            _proxyCount = _shape.GetChildCount();

            for (int i = 0; i < _proxyCount; ++i)
            {
                FixtureProxy proxy = _proxies[i];
                _shape.ComputeAABB(out proxy.aabb, ref xf, i);
                proxy.fixture = this;
                proxy.childIndex = i;
                proxy.proxyId = broadPhase.CreateProxy(ref proxy.aabb, proxy);

                _proxies[i] = proxy;
            }
        }

        internal void DestroyProxies(BroadPhase broadPhase)
        {
	        // Destroy proxies in the broad-phase.
	        for (int i = 0; i < _proxyCount; ++i)
	        {
                broadPhase.DestroyProxy(_proxies[i].proxyId);
		        _proxies[i].proxyId = BroadPhase.NullProxy;
	        }

	        _proxyCount = 0;
        }


        internal void Synchronize(BroadPhase broadPhase, ref Transform transform1, ref Transform transform2)
        {
            if (_proxyCount == 0)
            {
                return;
            }

            for (int i = 0; i < _proxyCount; ++i)
            {
                FixtureProxy proxy = _proxies[i];

                // Compute an AABB that covers the swept Shape (may miss some rotation effect).
                AABB aabb1, aabb2;
                _shape.ComputeAABB(out aabb1, ref transform1, proxy.childIndex);
                _shape.ComputeAABB(out aabb2, ref transform2, proxy.childIndex);

                proxy.aabb.Combine(ref aabb1, ref aabb2);

                Vector2 displacement = transform2.Position - transform1.Position;

                broadPhase.MoveProxy(proxy.proxyId, ref proxy.aabb, displacement);
            }

        }

        internal AABB _aabb;

        internal FixtureProxy[] _proxies;
        internal int _proxyCount;

        internal float _density;

	    internal Fixture _next;
	    internal Body _body;

	    internal Shape _shape;

	    internal float _friction;
	    internal float _restitution;

	    internal int _proxyId;
	    internal Filter _filter;

	    internal bool _isSensor;

	    internal object _userData;
    };

}
